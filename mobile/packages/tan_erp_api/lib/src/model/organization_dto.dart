//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/organization_branding_dto.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'organization_dto.g.dart';

/// OrganizationDto
///
/// Properties:
/// * [id] 
/// * [name] 
/// * [branding] 
@BuiltValue()
abstract class OrganizationDto implements Built<OrganizationDto, OrganizationDtoBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'name')
  String? get name;

  @BuiltValueField(wireName: r'branding')
  OrganizationBrandingDto? get branding;

  OrganizationDto._();

  factory OrganizationDto([void updates(OrganizationDtoBuilder b)]) = _$OrganizationDto;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(OrganizationDtoBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<OrganizationDto> get serializer => _$OrganizationDtoSerializer();
}

class _$OrganizationDtoSerializer implements PrimitiveSerializer<OrganizationDto> {
  @override
  final Iterable<Type> types = const [OrganizationDto, _$OrganizationDto];

  @override
  final String wireName = r'OrganizationDto';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    OrganizationDto object, {
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
    if (object.branding != null) {
      yield r'branding';
      yield serializers.serialize(
        object.branding,
        specifiedType: const FullType(OrganizationBrandingDto),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    OrganizationDto object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required OrganizationDtoBuilder result,
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
        case r'branding':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(OrganizationBrandingDto),
          ) as OrganizationBrandingDto?;
          if (valueDes == null) continue;
          result.branding.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  OrganizationDto deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = OrganizationDtoBuilder();
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


