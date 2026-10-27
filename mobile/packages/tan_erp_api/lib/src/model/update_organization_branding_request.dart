//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'update_organization_branding_request.g.dart';

/// UpdateOrganizationBrandingRequest
///
/// Properties:
/// * [primaryColor] 
/// * [logoFileId] 
@BuiltValue()
abstract class UpdateOrganizationBrandingRequest implements Built<UpdateOrganizationBrandingRequest, UpdateOrganizationBrandingRequestBuilder> {
  @BuiltValueField(wireName: r'primaryColor')
  String? get primaryColor;

  @BuiltValueField(wireName: r'logoFileId')
  String? get logoFileId;

  UpdateOrganizationBrandingRequest._();

  factory UpdateOrganizationBrandingRequest([void updates(UpdateOrganizationBrandingRequestBuilder b)]) = _$UpdateOrganizationBrandingRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UpdateOrganizationBrandingRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UpdateOrganizationBrandingRequest> get serializer => _$UpdateOrganizationBrandingRequestSerializer();
}

class _$UpdateOrganizationBrandingRequestSerializer implements PrimitiveSerializer<UpdateOrganizationBrandingRequest> {
  @override
  final Iterable<Type> types = const [UpdateOrganizationBrandingRequest, _$UpdateOrganizationBrandingRequest];

  @override
  final String wireName = r'UpdateOrganizationBrandingRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UpdateOrganizationBrandingRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.primaryColor != null) {
      yield r'primaryColor';
      yield serializers.serialize(
        object.primaryColor,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.logoFileId != null) {
      yield r'logoFileId';
      yield serializers.serialize(
        object.logoFileId,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    UpdateOrganizationBrandingRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UpdateOrganizationBrandingRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'primaryColor':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.primaryColor = valueDes;
          break;
        case r'logoFileId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.logoFileId = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UpdateOrganizationBrandingRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UpdateOrganizationBrandingRequestBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}


