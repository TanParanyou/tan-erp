//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'update_organization_profile_request.g.dart';

/// UpdateOrganizationProfileRequest
///
/// Properties:
/// * [name] 
/// * [nameEn] 
/// * [taxIdentifier] 
/// * [addressTh] 
/// * [addressEn] 
/// * [phone] 
@BuiltValue()
abstract class UpdateOrganizationProfileRequest implements Built<UpdateOrganizationProfileRequest, UpdateOrganizationProfileRequestBuilder> {
  @BuiltValueField(wireName: r'name')
  String? get name;

  @BuiltValueField(wireName: r'nameEn')
  String? get nameEn;

  @BuiltValueField(wireName: r'taxIdentifier')
  String? get taxIdentifier;

  @BuiltValueField(wireName: r'addressTh')
  String? get addressTh;

  @BuiltValueField(wireName: r'addressEn')
  String? get addressEn;

  @BuiltValueField(wireName: r'phone')
  String? get phone;

  UpdateOrganizationProfileRequest._();

  factory UpdateOrganizationProfileRequest([void updates(UpdateOrganizationProfileRequestBuilder b)]) = _$UpdateOrganizationProfileRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UpdateOrganizationProfileRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UpdateOrganizationProfileRequest> get serializer => _$UpdateOrganizationProfileRequestSerializer();
}

class _$UpdateOrganizationProfileRequestSerializer implements PrimitiveSerializer<UpdateOrganizationProfileRequest> {
  @override
  final Iterable<Type> types = const [UpdateOrganizationProfileRequest, _$UpdateOrganizationProfileRequest];

  @override
  final String wireName = r'UpdateOrganizationProfileRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UpdateOrganizationProfileRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.name != null) {
      yield r'name';
      yield serializers.serialize(
        object.name,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.nameEn != null) {
      yield r'nameEn';
      yield serializers.serialize(
        object.nameEn,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.taxIdentifier != null) {
      yield r'taxIdentifier';
      yield serializers.serialize(
        object.taxIdentifier,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.addressTh != null) {
      yield r'addressTh';
      yield serializers.serialize(
        object.addressTh,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.addressEn != null) {
      yield r'addressEn';
      yield serializers.serialize(
        object.addressEn,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.phone != null) {
      yield r'phone';
      yield serializers.serialize(
        object.phone,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    UpdateOrganizationProfileRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UpdateOrganizationProfileRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'name':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.name = valueDes;
          break;
        case r'nameEn':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.nameEn = valueDes;
          break;
        case r'taxIdentifier':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.taxIdentifier = valueDes;
          break;
        case r'addressTh':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.addressTh = valueDes;
          break;
        case r'addressEn':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.addressEn = valueDes;
          break;
        case r'phone':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.phone = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UpdateOrganizationProfileRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UpdateOrganizationProfileRequestBuilder();
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


