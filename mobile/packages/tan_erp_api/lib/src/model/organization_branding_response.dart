//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'organization_branding_response.g.dart';

/// OrganizationBrandingResponse
///
/// Properties:
/// * [primaryColor] 
/// * [logoFileId] 
/// * [logoUrl] 
/// * [rowVersion] 
@BuiltValue()
abstract class OrganizationBrandingResponse implements Built<OrganizationBrandingResponse, OrganizationBrandingResponseBuilder> {
  @BuiltValueField(wireName: r'primaryColor')
  String? get primaryColor;

  @BuiltValueField(wireName: r'logoFileId')
  String? get logoFileId;

  @BuiltValueField(wireName: r'logoUrl')
  String? get logoUrl;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  OrganizationBrandingResponse._();

  factory OrganizationBrandingResponse([void updates(OrganizationBrandingResponseBuilder b)]) = _$OrganizationBrandingResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(OrganizationBrandingResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<OrganizationBrandingResponse> get serializer => _$OrganizationBrandingResponseSerializer();
}

class _$OrganizationBrandingResponseSerializer implements PrimitiveSerializer<OrganizationBrandingResponse> {
  @override
  final Iterable<Type> types = const [OrganizationBrandingResponse, _$OrganizationBrandingResponse];

  @override
  final String wireName = r'OrganizationBrandingResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    OrganizationBrandingResponse object, {
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
    if (object.logoUrl != null) {
      yield r'logoUrl';
      yield serializers.serialize(
        object.logoUrl,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.rowVersion != null) {
      yield r'rowVersion';
      yield serializers.serialize(
        object.rowVersion,
        specifiedType: const FullType(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    OrganizationBrandingResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required OrganizationBrandingResponseBuilder result,
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
        case r'logoUrl':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.logoUrl = valueDes;
          break;
        case r'rowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rowVersion = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  OrganizationBrandingResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = OrganizationBrandingResponseBuilder();
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


