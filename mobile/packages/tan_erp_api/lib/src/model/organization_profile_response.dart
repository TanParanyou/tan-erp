//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'organization_profile_response.g.dart';

/// OrganizationProfileResponse
///
/// Properties:
/// * [id] 
/// * [name] 
/// * [nameEn] 
/// * [taxIdentifier] 
/// * [addressTh] 
/// * [addressEn] 
/// * [phone] 
/// * [rowVersion] 
@BuiltValue()
abstract class OrganizationProfileResponse implements Built<OrganizationProfileResponse, OrganizationProfileResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

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

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  OrganizationProfileResponse._();

  factory OrganizationProfileResponse([void updates(OrganizationProfileResponseBuilder b)]) = _$OrganizationProfileResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(OrganizationProfileResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<OrganizationProfileResponse> get serializer => _$OrganizationProfileResponseSerializer();
}

class _$OrganizationProfileResponseSerializer implements PrimitiveSerializer<OrganizationProfileResponse> {
  @override
  final Iterable<Type> types = const [OrganizationProfileResponse, _$OrganizationProfileResponse];

  @override
  final String wireName = r'OrganizationProfileResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    OrganizationProfileResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
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
    OrganizationProfileResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required OrganizationProfileResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
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
  OrganizationProfileResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = OrganizationProfileResponseBuilder();
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


