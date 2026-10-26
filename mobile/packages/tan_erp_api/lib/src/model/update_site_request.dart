//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'update_site_request.g.dart';

/// UpdateSiteRequest
///
/// Properties:
/// * [label] 
/// * [addressLine1] 
/// * [subdistrict] 
/// * [district] 
/// * [province] 
/// * [postalCode] 
/// * [countryCode] 
/// * [latitude] 
/// * [longitude] 
/// * [accessNote] 
@BuiltValue()
abstract class UpdateSiteRequest implements Built<UpdateSiteRequest, UpdateSiteRequestBuilder> {
  @BuiltValueField(wireName: r'label')
  String? get label;

  @BuiltValueField(wireName: r'addressLine1')
  String? get addressLine1;

  @BuiltValueField(wireName: r'subdistrict')
  String? get subdistrict;

  @BuiltValueField(wireName: r'district')
  String? get district;

  @BuiltValueField(wireName: r'province')
  String? get province;

  @BuiltValueField(wireName: r'postalCode')
  String? get postalCode;

  @BuiltValueField(wireName: r'countryCode')
  String? get countryCode;

  @BuiltValueField(wireName: r'latitude')
  double? get latitude;

  @BuiltValueField(wireName: r'longitude')
  double? get longitude;

  @BuiltValueField(wireName: r'accessNote')
  String? get accessNote;

  UpdateSiteRequest._();

  factory UpdateSiteRequest([void updates(UpdateSiteRequestBuilder b)]) = _$UpdateSiteRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UpdateSiteRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UpdateSiteRequest> get serializer => _$UpdateSiteRequestSerializer();
}

class _$UpdateSiteRequestSerializer implements PrimitiveSerializer<UpdateSiteRequest> {
  @override
  final Iterable<Type> types = const [UpdateSiteRequest, _$UpdateSiteRequest];

  @override
  final String wireName = r'UpdateSiteRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UpdateSiteRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.label != null) {
      yield r'label';
      yield serializers.serialize(
        object.label,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.addressLine1 != null) {
      yield r'addressLine1';
      yield serializers.serialize(
        object.addressLine1,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.subdistrict != null) {
      yield r'subdistrict';
      yield serializers.serialize(
        object.subdistrict,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.district != null) {
      yield r'district';
      yield serializers.serialize(
        object.district,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.province != null) {
      yield r'province';
      yield serializers.serialize(
        object.province,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.postalCode != null) {
      yield r'postalCode';
      yield serializers.serialize(
        object.postalCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.countryCode != null) {
      yield r'countryCode';
      yield serializers.serialize(
        object.countryCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.latitude != null) {
      yield r'latitude';
      yield serializers.serialize(
        object.latitude,
        specifiedType: const FullType.nullable(double),
      );
    }
    if (object.longitude != null) {
      yield r'longitude';
      yield serializers.serialize(
        object.longitude,
        specifiedType: const FullType.nullable(double),
      );
    }
    if (object.accessNote != null) {
      yield r'accessNote';
      yield serializers.serialize(
        object.accessNote,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    UpdateSiteRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UpdateSiteRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'label':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.label = valueDes;
          break;
        case r'addressLine1':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.addressLine1 = valueDes;
          break;
        case r'subdistrict':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.subdistrict = valueDes;
          break;
        case r'district':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.district = valueDes;
          break;
        case r'province':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.province = valueDes;
          break;
        case r'postalCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.postalCode = valueDes;
          break;
        case r'countryCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.countryCode = valueDes;
          break;
        case r'latitude':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.latitude = valueDes;
          break;
        case r'longitude':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.longitude = valueDes;
          break;
        case r'accessNote':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.accessNote = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UpdateSiteRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UpdateSiteRequestBuilder();
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


